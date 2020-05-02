UPDATE Users SET
    PhoneNumberConfirmed = @PhoneNumberConfirmed
WHERE
    ID = @ID