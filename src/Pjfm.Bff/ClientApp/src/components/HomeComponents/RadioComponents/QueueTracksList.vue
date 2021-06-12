<template>
  <div>
    <span class="overline grey--text">{{spanTitle}}</span><br>
    <v-list subheader two-line>
      <div v-for="(item, i) in tracks" :key="i">
        <v-list-item>
          <v-list-item-content>
            <v-list-item-title>
              {{i + 1}}. {{item.track.title}}
            </v-list-item-title>
            <v-list-item-subtitle :style="`margin-left: ${i < 9? '17': '25'}px;`">{{item.track.artists.join(", ")}}</v-list-item-subtitle>
            <div v-if="item.message !== undefined" class="body-2 blue--text" :style="`margin-left: ${i < 9? '17': '25'}px;`">
              {{item.user}} stuurde: {{ item.message }}
            </div>
          </v-list-item-content>
          <v-list-item-icon v-if="breakPointWidth > 600">
            <v-chip :class="item.chipClass" outlined>
              <v-icon left>{{item.icon}}</v-icon>
              {{item.user}}
            </v-chip>
          </v-list-item-icon>
          <div v-else :class="`${item.chipClass.split(' ')[1]}`">{{ item.user }}</div>
          <v-list-item-action v-if="isMod">
            <v-btn color="red" text x-small :disabled="false" @click="removeTrack(item.id)">Remove</v-btn>
          </v-list-item-action>
        </v-list-item>
        <v-divider v-if="i !== tracks.length -1"></v-divider>
      </div>
      <v-list-item v-if="tracks.length <= 0 && emptyMessage !== null">
        <v-list-item-content>
          <v-list-item-title>
            {{ emptyMessage }}
          </v-list-item-title>
        </v-list-item-content>
      </v-list-item>
    </v-list>
  </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import {Prop} from "vue-property-decorator";
import {trackDto} from "@/common/types";

@Component({
  name: 'QueueTracksList',
})
export default class QueueTracksList extends Vue {
  @Prop({type: Array, required: true}) readonly tracks !: Array<trackDto>;
  @Prop({type: String, required: true}) readonly spanTitle !: string;
  @Prop() readonly emptyMessage !: string | null;
  
  get breakPointWidth():number{
    // @ts-ignore
    return this.$vuetify.breakpoint.width;
  }

  get isMod():boolean{
    return this.$store.getters['profileModule/isMod'];
  }
  
  private removeTrack(trackId: string){
    // @ts-ignore
    this.$axios.put(`api/playback/mod/dequeueTrack?trackId=${trackId}`);
  }
}
</script>

<style scoped>

</style>