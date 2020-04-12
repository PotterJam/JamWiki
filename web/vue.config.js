module.exports = {
  devServer: {
    proxy: {
      "^/api": {
        target: "http://localhost:5000",
        ws: true,
        changeOrigin: true
      }
    }
  },
  transpileDependencies: ["vuetify", "store"],
  configureWebpack: {
    optimization: {
      splitChunks: {
        minSize: 10000,
        maxSize: 240000
      }
    }
  }
};
