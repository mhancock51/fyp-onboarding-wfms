export default interface AuthenticatedUser {
    id: string;
    displayName: string;
    emailAddress: string;    
    isSupervisor: string;
    departmentId: string;
    departmentName: string;
    organisationId: string;
    accountStatus: string;
    jwtToken: string;
}