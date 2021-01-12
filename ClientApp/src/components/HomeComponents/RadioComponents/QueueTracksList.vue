<template>
  <div>
    <span class="overline grey--text">Filler watchtrij</span><br>
    <v-list>
      <v-list-item-group>
        <v-list-item
            v-for="(item, i) in tracks"
            :key="i"
        >
          <v-list-item-content>
            <v-list-item-title>
              {{i + 1}}. {{item.track.artists[0]}} - {{item.track.title}}
            </v-list-item-title>
          </v-list-item-content>
          <v-list-item-icon>
            <v-chip
                :class="item.chipClass"
                outlined
            >
              <v-icon left>{{item.icon}}</v-icon>
              {{item.user}}
            </v-chip>
          </v-list-item-icon>
        </v-list-item>
        <v-list-item v-if="tracks.length <= 0 && emptyMessage !== null">
          <v-list-item-content>
            <v-list-item-title>
              {{ emptyMessage }}
            </v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list-item-group>
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
}
</script>

<style scoped>

</style>