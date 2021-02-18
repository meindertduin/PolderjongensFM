<template>
  <div>
    <v-row justify="center">
      <v-col cols="11" md="10">
        <SearchBox />
      </v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import SearchBox from "@/components/SearchComponents/SearchBox.vue";

@Component({
  name: 'Search',
  components: {
    SearchBox,
  }
})
export default class Search extends Vue {

}
</script>