const fs = require('fs');
const path = require('path');

module.exports = {
  "transpileDependencies": [
    "vuetify"
  ],
  devServer: {
    hotOnly: false,
    port: 8085,
    host: '0.0.0.0',
    public: "https://localhost:8085",
  }
}
