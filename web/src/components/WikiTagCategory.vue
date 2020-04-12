<template>
  <div class="hello">
    <h1>{{ tag }}</h1>
  </div>
</template>

<script>
export default {
  name: "WikiTagCategory",
  props: {
    tag: String
  }
};
</script>