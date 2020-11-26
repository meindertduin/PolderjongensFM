import {userSettings} from "@/common/types";

export const defaultSettings: userSettings = {
    darkMode: window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches,    
}

    
    
