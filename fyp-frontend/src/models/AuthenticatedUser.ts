export default interface AuthenticatedUser {
    id: string;
    displayName: string;
    emailAddress: string;    
    isSupervisor: boolean;
    departmentId: string;
    departmentName: string;
    organisationId: string;
    accountStatus: string;
    jwtToken: string;
}