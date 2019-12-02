<template>
    <div>
        <h1 class="pb-5"> Tags </h1>
        <div v-for="tag in tags" v-bind:key="tag">
            <v-btn class="ma-2" color="secondary" v-on:click="gotoTagPage(tag)">{{ tag }}</v-btn>
        </div>
    </div>
</template>

<script>
export default {
    data () {
        return {
            index: -1,
            tags: []
        }
    },
    created() {
        this.axios.get(`http://localhost:5000/api/wiki/tags`)
        .then(response => {
            this.tags = response.data
        });
    },
    methods: {
        gotoTagPage(tag) {
            this.$router.go('/wikiswithtag/' + tag);
        }
    }
}
</script>