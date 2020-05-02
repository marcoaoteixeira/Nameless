DELETE FROM UserClaims
WHERE
    UserID = @UserID
AND Type = @OldClaimType;

INSERT INTO UserClaims VALUES (
    @UserID,
    @NewClaimType,
    @NewClaimValue
);