export interface profileModel{
    userName: string,
    email: string,
}

export interface userSettings{
    darkMode: boolean,
}

export interface liveChatMessageModel{
    userName: string,
    message: string,
    timeSend: string,
}

export interface trackDto{
    id: string,
    title: string,
    artists: string[],
    term: number,
    trackType: number,
    songDurationMs: number,
    applicationUserId: string,
}
    