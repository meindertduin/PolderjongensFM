<template>
    <div class="chat-input">
        <v-textarea v-model="messageInput" filled auto-grow label="Doe een verzoekje!" rows="2" row-height="20"></v-textarea>
        <v-btn @click="sendMessage" class="send-button" color="green">Send</v-btn>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'

    @Component({
        name: 'MessageInput',
    })
    export default class MessageInput extends Vue {
        private messageInput:string = "";
        
        sendMessage(){
            this.$parent.sendMessage(this.messageInput);
            this.messageInput = "";
        }
    }
</script>

<style scoped>
    .chat-input{
        padding: 5px;
    }
    .send-button{
        width: 100%;
        top: -25px;
    }
</style>