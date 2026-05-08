-- Create Booking Request Procedure
CREATE OR REPLACE PROCEDURE sp_create_booking_request(
    p_reference TEXT,
    p_customer_id UUID,
    p_vendor_id UUID,
    p_product_id INTEGER,
    p_event_date TIMESTAMP,
    p_event_time TEXT,
    p_location TEXT,
    p_guest_count INTEGER,
    p_requirements TEXT,
    p_budget TEXT,
    p_notes TEXT,
    p_total_amount DECIMAL,
    OUT p_booking_id INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Conflict Check
    IF EXISTS (
        SELECT 1 FROM "VendorAvailabilities" 
        WHERE "VendorId" = p_vendor_id 
        AND DATE("BlockedDate") = DATE(p_event_date)
    ) THEN
        RAISE EXCEPTION 'This date is no longer available for the selected vendor.';
    END IF;

    INSERT INTO "Bookings" (
        "BookingReference", "CustomerId", "VendorId", "ProductId", 
        "EventDate", "EventTime", "Location", "GuestCount", 
        "Requirements", "Budget", "Notes", "Status", "PaymentStatus", 
        "TotalAmount", "CreatedAt", "UpdatedAt"
    )
    VALUES (
        p_reference, p_customer_id, p_vendor_id, p_product_id, 
        p_event_date, p_event_time, p_location, p_guest_count, 
        p_requirements, p_budget, p_notes, 1, 0, -- 1 = Pending, 0 = None
        p_total_amount, NOW(), NOW()
    )
    RETURNING "Id" INTO p_booking_id;

    -- Initial History Entry
    INSERT INTO "BookingStatusHistories" (
        "BookingId", "OldStatus", "NewStatus", "ChangedBy", "ChangedAt", "Notes"
    )
    VALUES (
        p_booking_id, 0, 1, p_customer_id::TEXT, NOW(), 'Initial request submitted.'
    );
END;
$$;

-- Approve Booking Procedure
CREATE OR REPLACE PROCEDURE sp_approve_booking(
    p_booking_id INTEGER,
    p_vendor_id UUID,
    p_notes TEXT
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_old_status INTEGER;
    v_event_date TIMESTAMP;
BEGIN
    -- Validate Booking exists and belongs to Vendor
    SELECT "Status", "EventDate" INTO v_old_status, v_event_date 
    FROM "Bookings" 
    WHERE "Id" = p_booking_id AND "VendorId" = p_vendor_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Booking not found or access denied.';
    END IF;

    -- Validate State Transition (Must be Pending or UnderReview)
    IF v_old_status NOT IN (1, 2) THEN
        RAISE EXCEPTION 'Invalid state transition. Booking must be Pending or UnderReview.';
    END IF;

    -- Update Booking Status to Confirmed (4) and PaymentStatus to Pending (1)
    UPDATE "Bookings"
    SET "Status" = 4, 
        "PaymentStatus" = 1,
        "UpdatedAt" = NOW()
    WHERE "Id" = p_booking_id;

    -- Reserve Slot in Availability
    INSERT INTO "VendorAvailabilities" ("VendorId", "BlockedDate", "Reason", "CreatedAt")
    VALUES (p_vendor_id, v_event_date, 'Confirmed Booking: ' || p_booking_id, NOW());

    -- History Entry
    INSERT INTO "BookingStatusHistories" (
        "BookingId", "OldStatus", "NewStatus", "ChangedBy", "ChangedAt", "Notes"
    )
    VALUES (
        p_booking_id, v_old_status, 4, p_vendor_id::TEXT, NOW(), p_notes
    );
END;
$$;

-- Reject Booking Procedure
CREATE OR REPLACE PROCEDURE sp_reject_booking(
    p_booking_id INTEGER,
    p_vendor_id UUID,
    p_notes TEXT
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_old_status INTEGER;
BEGIN
    SELECT "Status" INTO v_old_status 
    FROM "Bookings" 
    WHERE "Id" = p_booking_id AND "VendorId" = p_vendor_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Booking not found or access denied.';
    END IF;

    UPDATE "Bookings"
    SET "Status" = 5, -- 5 = Rejected
        "UpdatedAt" = NOW()
    WHERE "Id" = p_booking_id;

    INSERT INTO "BookingStatusHistories" (
        "BookingId", "OldStatus", "NewStatus", "ChangedBy", "ChangedAt", "Notes"
    )
    VALUES (
        p_booking_id, v_old_status, 5, p_vendor_id::TEXT, NOW(), p_notes
    );
END;
$$;

-- Confirm Payment Procedure
CREATE OR REPLACE PROCEDURE sp_confirm_payment(
    p_booking_id INTEGER,
    p_customer_id UUID
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_old_status INTEGER;
    v_old_payment_status INTEGER;
BEGIN
    SELECT "Status", "PaymentStatus" INTO v_old_status, v_old_payment_status
    FROM "Bookings" 
    WHERE "Id" = p_booking_id AND "CustomerId" = p_customer_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Booking not found or access denied.';
    END IF;

    -- Must be Confirmed and PaymentPending
    IF v_old_status != 4 OR v_old_payment_status != 1 THEN
        RAISE EXCEPTION 'Booking must be Confirmed and awaiting payment.';
    END IF;

    UPDATE "Bookings"
    SET "Status" = 8, -- 8 = Booked
        "PaymentStatus" = 3, -- 3 = Paid
        "UpdatedAt" = NOW()
    WHERE "Id" = p_booking_id;

    INSERT INTO "BookingStatusHistories" (
        "BookingId", "OldStatus", "NewStatus", "ChangedBy", "ChangedAt", "Notes"
    )
    VALUES (
        p_booking_id, v_old_status, 8, p_customer_id::TEXT, NOW(), 'Payment confirmed by customer.'
    );
END;
$$;

-- Request Booking Modification Procedure
CREATE OR REPLACE PROCEDURE sp_request_booking_modification(
    p_booking_id INTEGER,
    p_vendor_id UUID,
    p_notes TEXT
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_old_status INTEGER;
BEGIN
    SELECT "Status" INTO v_old_status 
    FROM "Bookings" 
    WHERE "Id" = p_booking_id AND "VendorId" = p_vendor_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Booking not found or access denied.';
    END IF;

    UPDATE "Bookings"
    SET "Status" = 3, -- 3 = ModificationRequested
        "UpdatedAt" = NOW()
    WHERE "Id" = p_booking_id;

    INSERT INTO "BookingStatusHistories" (
        "BookingId", "OldStatus", "NewStatus", "ChangedBy", "ChangedAt", "Notes"
    )
    VALUES (
        p_booking_id, v_old_status, 3, p_vendor_id::TEXT, NOW(), p_notes
    );
END;
$$;

-- Cancel Booking Procedure
CREATE OR REPLACE PROCEDURE sp_cancel_booking(
    p_booking_id INTEGER,
    p_user_id UUID,
    p_notes TEXT
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_old_status INTEGER;
    v_vendor_id UUID;
    v_event_date TIMESTAMP;
BEGIN
    -- Get current state
    SELECT "Status", "VendorId", "EventDate" INTO v_old_status, v_vendor_id, v_event_date
    FROM "Bookings" 
    WHERE "Id" = p_booking_id AND ("CustomerId" = p_user_id OR "VendorId" = p_user_id);

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Booking not found or access denied.';
    END IF;

    -- Update status to Cancelled (10)
    UPDATE "Bookings"
    SET "Status" = 10,
        "UpdatedAt" = NOW()
    WHERE "Id" = p_booking_id;

    -- Remove from Availability if it was a Confirmed Booking
    DELETE FROM "VendorAvailabilities"
    WHERE "VendorId" = v_vendor_id 
    AND DATE("BlockedDate") = DATE(v_event_date)
    AND "Reason" LIKE 'Confirmed Booking: ' || p_booking_id || '%';

    -- History Entry
    INSERT INTO "BookingStatusHistories" (
        "BookingId", "OldStatus", "NewStatus", "ChangedBy", "ChangedAt", "Notes"
    )
    VALUES (
        p_booking_id, v_old_status, 10, p_user_id::TEXT, NOW(), p_notes
    );
END;
$$;
