<template>
  <h1>Wikis with tag {{ $route.params.tagName }}</h1>
</template>

<script>
export default {
  name: "WikiTags",
  data() {
    return {
      tagName: null
    };
  }
};
</script>
