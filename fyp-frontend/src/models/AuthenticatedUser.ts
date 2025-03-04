export default interface AuthenticatedUser {
    id: string;
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