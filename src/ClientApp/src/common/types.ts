export interface applicationUser {
    id: string,
    displayName: string,
    member: boolean,
    spotifyAuthenticated: boolean,
}

export interface userSettings{
    darkMode: boolean,
}

export interface modLocalSettings {
    requestAsMod: boolean,
}

export interface liveChatMessageModel{
    userName: string,
    message: string,
    timeSend: string,
}

export interface trackDto{
    id: string,
    title: string,
    mainArtistId: string,
    artists: string[],
    term: number,
    trackType: number,
    songDurationMs: number,
    user: applicationUser,
    message: string,
}

export enum playbackState{
    'DJ Only Mode',
    'Verzoekjes (op volgorde)',
    'Verzoekjes (random)',
    'Round-Robin'
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
    fillerQueueState: fillerQueueType,
    browserQueueSettings: browserQueueSettings, 
}

export interface browserQueueSettings {
    genres: string[],
    seedTracks: string[]
    seedArtists: string[],
    tempo: queueSettingsValue,
    instrumentalness: queueSettingsValue,
    popularity: queueSettingsValue,
    energy: queueSettingsValue,
    danceAbility: queueSettingsValue,
    valence: queueSettingsValue,
}

export enum fillerQueueType {
    userTopTracks,
    genreBrowsing
}

export enum queueSettingsValue {
    not,
    minimal,
    little,
    average,
    much,
    maximal,
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

export interface identityProfile {
    userProfile: applicationUser,
    isMod: boolean,
    isSpotifyAuthenticated: boolean,
    emailConfirmed: boolean,
}

export interface queueTrack {
    id: string,
    track: trackDto,
    user: string,
    icon: string,
    queueNum: number,
    chipClass: string,
    message: string | undefined,
}