
export interface User {
    id: string,
    roles: UserRole[],
    username: string,
    emailConfirmed: boolean,
    spotifyAuthenticated: boolean,
}

export enum UserRole {
    User = 0,
    Mod = 1,
}
