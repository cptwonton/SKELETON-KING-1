namespace PUZZLEBOX;

public enum AccountType
{
    Disabled = 0,
    Normal = 3,
    Legacy = 4,    // User prepaid for their account during the Beta.
    Staff = 5,

    RankedMatchHost = 100,  // allowed to host both ranked and unranked matches
    UnrankedMatchHost = 101 // only allowed to host unranked (public and bot) matches
}
