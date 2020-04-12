<template>
  <div>
    <tiptap-vuetify
      :value="body"
      v-on:input="$emit('update:body', $event)"
      :extensions="extensions" placeholder="Write something …"
    />
  </div>
</template>

<script>
import {
  // eslint-disable-next-line no-unused-vars
  TiptapVuetify,
  Heading,
  Bold,
  Italic,
  Underline,
  Code,
  CodeBlock,
  BulletList,
  OrderedList,
  ListItem,
  Link,
  Blockquote,
  HardBreak,
  HorizontalRule,
  History,
  TodoList,
  TodoItem
} from 'tiptap-vuetify'

export default {
    name: 'WikiEditor',
    props: {
      body: String
    },
    components: {
      TiptapVuetify
    },
    data() {
      return {
        extensions: [
        Blockquote,
        Link,
        Bold,
        Underline,
        Italic,
        ListItem, // if you need to use a list (BulletList, OrderedList)
        BulletList,
        OrderedList,
        [
          Heading,
          {
            // Options that fall into the tiptap's extension
            options: {
              levels: [1, 2]
            }
          }
        ],
        Link,
        Code,
        CodeBlock,
        HorizontalRule,
        TodoList,
        [TodoItem, {
          options: {
            nested: true
          }
        }],
        HardBreak, // line break on Shift + Ctrl + Enter
        History,
        ]
      }
    }
}
</script>