import Vue from 'vue';
// @ts-ignore
import Vuetify from 'vuetify/lib';

Vue.use(Vuetify);

// @ts-ignore
import colors from 'vuetify/lib/util/colors'

export default new Vuetify({
    theme: {
        themes: {
            light: {
                primary: colors.purple,
                secondary: colors.grey.darken1,
                accent: colors.shades.black,
                error: colors.red.accent3,
                background: colors.indigo.lighten5
            },
            dark: {
                primary: colors.orange,
                background: colors.indigo.base
            }
        }
    }
});
