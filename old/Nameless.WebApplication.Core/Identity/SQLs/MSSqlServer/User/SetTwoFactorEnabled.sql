UPDATE Users SET
    TwoFactorEnabled = @TwoFactorEnabled
WHERE
    ID = @ID