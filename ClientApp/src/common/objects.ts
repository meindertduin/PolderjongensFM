import {modLocalSettings, userSettings} from "@/common/types";

export const defaultSettings: userSettings = {
    darkMode: true,
    // window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches,    
}

export const defaultModLocalSettings: modLocalSettings = {
    requestAsMod: false,
}

export const emailPattern = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

export const formRules = {
    required: (value:any) => !!value || 'verplicht veld',
    counterMax100: (value:any) => value.length <= 100 || 'Maximaal 100 tekens',
    counterMax200: (value:any) => value.length <= 200 || 'Maximaal 200 tekens',
    counterMax2000: (value:any) => value.length <= 2000 || 'Maximaal 2000 tekens',
    email: (value:any) => {
        return emailPattern.test(value) || 'Ongeldig email'
    },
}    
    
