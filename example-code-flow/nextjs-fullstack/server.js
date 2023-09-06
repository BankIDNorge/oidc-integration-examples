const { createServer } = require("https");
const { parse } = require("url");
const next = require("next");
const fs = require("fs");
const port = process.env.PORT && parseInt(process.env.PORT) || 3000;
const dev = process.env.NODE_ENV !== "production";
const app = next({ dev });
const handle = app.getRequestHandler();

const httpsOptions = {
  key: fs.readFileSync("./certs/localhost-key.pem"),
  cert: fs.readFileSync("./certs/localhost.pem")
};

app.prepare().then(() => {
  createServer(httpsOptions, async (req, res) => {
    const parsedUrl = parse(req.url, true);
    await handle(req, res, parsedUrl);
  }).listen(port, (err) => {
    if (err) throw err;
    console.log(`- info Ready on https://localhost:${port}`);
  });
});
