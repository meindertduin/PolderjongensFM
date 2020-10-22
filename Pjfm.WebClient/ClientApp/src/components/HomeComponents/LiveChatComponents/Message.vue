<template>
    <div class="message">
        <v-alert border="top" color="secondary">
            <div class="message-top">
                <div class="message-userName">{{messageContent.userName}}</div>
                <div class="message-date">{{messageContent.timeSend}}</div>
            </div>
            {{messageContent.message}}
        </v-alert>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'
    import {Prop} from 'vue-property-decorator'
    import {liveChatMessageModel} from "@/common/types";

    @Component({
        name: 'Message',
    })
    export default class Message extends Vue {
        @Prop({type: Object, required: true}) readonly messageContent !: liveChatMessageModel;
    }
</script>

<style scoped>
    .message-top{
        display: flex;
    }
    
    .message-userName{
        margin-right: 5px;
        font-weight: bold;
        font-size: large;
    }
    .message-date{
        font-size: smaller;
    }
</style>