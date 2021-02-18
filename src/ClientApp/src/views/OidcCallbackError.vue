<template>
    <div>
        <h1>Er ging iets fout bij het authentiseren</h1>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'


    @Component({
        name: 'OidcCallbackError',
    })
    export default class OidcCallbackError extends Vue{
        
    }
</script>

<style scoped>

</style>