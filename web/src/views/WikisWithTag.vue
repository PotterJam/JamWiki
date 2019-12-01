<template>
    <h1>Wikis with tag {{ $route.params.tagName }}</h1>
</template>

<script>
export default {
    data () {
        return {
            tagName: null
        }
    }
}
</script>