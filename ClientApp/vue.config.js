const fs = require('fs');
const path = require('path');

module.exports = {
  "transpileDependencies": [
    "vuetify"
  ],
  devServer: {
    https: {
      key: fs.readFileSync(path.relative(__dirname, 'server.key')),
      cert: fs.readFileSync(path.relative(__dirname, 'server.cert')),
    }
  }
  
}