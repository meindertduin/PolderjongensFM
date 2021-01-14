const fs = require('fs');
const path = require('path');

module.exports = {
  "transpileDependencies": [
    "vuetify"
  ],
  configureWebpack: config => {
    if (process.env.NODE_ENV !== 'production'){
      config.devServer = {
        hotOnly: false,
        port: 8085,
        host: '0.0.0.0',
        public: "https://localhost:8085",
        https: {
          key: fs.readFileSync(path.relative(__dirname, 'server.key')),
          cert: fs.readFileSync(path.relative(__dirname, 'server.cert')),
        },
      }
    }
  },
}
