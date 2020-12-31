<template>
  <v-row>
    <v-col class="d-block">
      <v-card
          class="pa-2"
          outlined
          round
      >
        <span class="overline grey--text">Live Chat</span><br>
          <div class="message-container" id="livechat-messagebox">
            <Message v-for="(messageModel, index) in messages" :key="index" :message-content="messageModel" />
          </div>
          <div v-if="oidcAuthenticated" class="chat-input-container">
            <MessageInput />
          </div>
          <div v-else class="chat-input-container">
            <v-row justify="center" align-content="center">
              <v-btn @click="signInOidcClient" color="primary">Login om mee te chatten</v-btn>
            </v-row>
          </div>
      </v-card>
  </v-col>
  </v-row>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'
    import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
    import Message from "@/components/HomeComponents/LiveChatComponents/Message.vue";
    import MessageInput from "@/components/HomeComponents/LiveChatComponents/MessageInput.vue";
    import {liveChatMessageModel} from "@/common/types";
    import {Watch} from "vue-property-decorator";
    
    @Component({
        name: 'Livechat',
        components: {
            Message,
            MessageInput,
        },
    })
    export default class Livechat extends Vue {

        private socketConnection: HubConnection | null = null;
        private messages: Array<liveChatMessageModel> = [];

        @Watch('messages')
        onMessageAdded(newValue:Array<liveChatMessageModel>, oldValue:Array<liveChatMessageModel>){
            setTimeout(() => {
                this.messageBoxScrollToBottom();
            }, 50);
        }
        
        get oidcAuthenticated(){
            return this.$store.getters['oidcStore/oidcIsAuthenticated'];
        }
        
        created(){
            this.connect()
        }

        mounted(){
            this.messageBoxScrollToBottom();
        }
        
        connect(){
            this.socketConnection = new HubConnectionBuilder()
                .withUrl(`${process.env.VUE_APP_API_BASE_URL}/livechat`)
                .build();

            this.socketConnection.start()
                .then(() => {
                    this.socketConnection?.on("ReceiveMessage", (message: liveChatMessageModel) => {
                        this.addMessage(message);
                    });
                    
                    this.socketConnection?.on("LoadMessages", (messages: Array<liveChatMessageModel>) => {
                        messages.forEach(m => this.addMessage(m));
                    })
                });
        }
        
        addMessage(message: liveChatMessageModel):void{
            const options = {hour: "numeric", minute: "numeric"};
            message.timeSend = Intl.DateTimeFormat("nl-NL", options).format(Date.parse(message.timeSend));
            this.messages.push(message);
        }

        messageBoxScrollToBottom(){
            let messageBox = this.$el.querySelector("#livechat-messagebox");
            if (messageBox){
              messageBox.scrollTop = messageBox.scrollHeight;
            }
        }
        
        sendMessage(message: string){
            if (this.oidcAuthenticated === false) return;
            return this.socketConnection?.invoke("SendMessage", message);
        }
        
        private signInOidcClient(){
            this.$store.dispatch('oidcStore/authenticateOidc');
        }
        
    }
</script>

<style scoped>
    .livechat-container{
        padding: 20px;
        border-radius: 20px;
        -webkit-box-shadow: 3px 3px 23px 0px rgba(0,0,0,0.36);
        -moz-box-shadow: 3px 3px 23px 0px rgba(0,0,0,0.36);
        box-shadow: 3px 3px 23px 0px rgba(0,0,0,0.36);
        height: 500px;
    }
    
    .message-container{
        height: 300px;
        overflow-y: scroll;
        overflow-x: hidden;
        padding: 5px;
    }
    
    .chat-input-container{
        
    }
</style>