<template>
    <div class="chat-input">
        <v-textarea v-model="messageInput" counter="200" filled auto-grow label="Doe een verzoekje!" rows="2" row-height="20" :rules="[charsMaxFormRule200]">
            
        </v-textarea>
        <v-btn @click="sendMessage" :disabled="messageInput.length > 200" class="send-button" color="orange">Bericht versturen</v-btn>
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
        
        private charsMaxFormRule200 = (value:any) => value.length <= 2000 || 'Maximaal 2000 tekens';


        sendMessage(){
            if (this.messageInput.length > 200) return;
            
            // @ts-ignore
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
        top: -10px;
    }
</style>