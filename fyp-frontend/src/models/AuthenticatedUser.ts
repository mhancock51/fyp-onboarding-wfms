export default interface AuthenticatedUser {
    accountId: string;
    displayName: string;
    emailAddress: string;
    isOnboarder: string;
    isAdmin: string;
    departmentId: string;
    departmentName: string;
    organisationId: string;
    accountStatus: string;
    jwtToken: string;
}