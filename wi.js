const express = require("express");
const app = express();
app.get("/",(req,res)=>{
    res.send("aaa");
})
app.listen(3000,()=>{
    console.log("aaa")
})