import{o as n,c as m,b as o,J as y,q as u,k as _,K as l,L as p,P as x,N as g,O as h,Q as V}from"./app.8a61673d.js";const B={preserveAspectRatio:"xMidYMid meet",viewBox:"0 0 24 24",width:"1.2em",height:"1.2em"},M=o("path",{fill:"currentColor",d:"M4 22h12v-2H4V8H2v12c0 1.103.897 2 2 2z"},null,-1),T=o("path",{fill:"currentColor",d:"M20 2H8c-1.103 0-2 .897-2 2v12c0 1.103.897 2 2 2h12c1.103 0 2-.897 2-2V4c0-1.103-.897-2-2-2zm-2 9h-3v3h-2v-3h-3V9h3V6h2v3h3v2z"},null,-1),k=[M,T];function z(r,d){return n(),m("svg",B,k)}const C={name:"bxs-add-to-queue",render:z},N=y("posts",()=>{const r=u([]),d=u(),v=_(()=>r.value),f=async a=>(await c(),r.value.find(e=>e.path===a)),s=async()=>{let a=["todos"],e=r.value.filter(i=>a.includes(i.path)),t=r.value.filter(i=>!a.includes(i.path));return e.concat(t)},c=async a=>{var t,i;d.value=a;const e=await l.api(new p);e.succeeded&&(r.value=(i=(t=e.response)==null?void 0:t.results)!=null?i:[],r.value=await s())};return{error:d,allPosts:v,toggleDraftPost:async a=>{const e=r.value.find(i=>i.path===a);e.draft=!e.draft;let t=await l.api(new h(e));await c(t.error)},getPost:f,refreshPosts:c,addPost:async a=>{let e={id:a==null?void 0:a.id,mdText:a==null?void 0:a.mdText,title:a==null?void 0:a.title,path:a==null?void 0:a.path,summary:a==null?void 0:a.summary,draft:a==null?void 0:a.draft};r.value.push(new x(e));let t=await l.api(new g(e));await c(t.error)},removePost:async a=>{r.value=r.value.filter(t=>t.id!=a);let e=await l.api(new V({id:a}));await c(e.error)},updatePost:async a=>{let e={id:a==null?void 0:a.id,mdText:a==null?void 0:a.mdText,title:a==null?void 0:a.title,path:a==null?void 0:a.path,summary:a==null?void 0:a.summary,draft:a==null?void 0:a.draft},t=await l.api(new h(e));await c(t.error)},postsOrdered:s}});export{C as A,N as u};
