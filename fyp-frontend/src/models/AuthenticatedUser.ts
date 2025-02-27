export default interface AuthenticatedUser {
    AccountId: string;
    DisplayName: string;
    EmailAddress: string;
    IsOnboarder: string;
    IsAdmin: string;
    DepartmentId: string;
    OrganisationId: string;
    AccountStatus: string;
    JwtToken: string;
}