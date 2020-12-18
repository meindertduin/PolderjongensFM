export interface applicationUser {
    id: string,
    displayName: string,
    email: string,
    member: boolean,
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

export interface playbackSettings {
    isPlaying: boolean,
    playbackTermFilter: number,
    playbackState: playbackState,
}

export enum playbackState{
    'DJ Only Mode',
    'Verzoekjes (op volgorde)',
    'Verzoekjes (random)',
}

export interface hubServerMessage{
    message: string,
    error: boolean,
}

export interface playbackSettings {
    isPlaying: boolean,
    playbackTermFilter: number,
    includedUsers: Array<applicationUser>,
    playbackState: playbackState,
    maxRequestsPerUser: number,
}

export interface userPlaybackInfo {
    secondaryQueuedTracks: Array<trackDto>,
    priorityQueuedTracks: Array<trackDto>,
    fillerQueuedTracks: Array<trackDto>,
    startingTime: string,
    currentPlayingTrack: trackDto,
}

export interface alertInfo {
    type: string,
    message: string,
}

export interface djPlaybackInfo {
    currentPlayingTrack: trackDto,
    startingTime: string,
    fillerQueuedTracks: Array<trackDto>,
    secondaryQueuedTracks: Array<trackDto>,
    priorityQueuedTracks: Array<trackDto>,
}

export interface userPlaybackSettings {
    playbackState: playbackState,
    isPlaying: boolean,
    maxRequestsPerUser: number,
}
