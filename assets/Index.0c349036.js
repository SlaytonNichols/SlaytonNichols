import{_ as x}from"./AppBreadcrumb.vue_vue_type_script_setup_true_lang.b64d0088.js";import{d as v,e as g,q as k,H as C,o,c as n,b as c,a as i,F as w,h as A,u as B,g as u,l as b,I as N,Q as V,w as P,i as R,t as _,J as j,r as E}from"./app.630e30ca.js";import"./home.893413b7.js";const F={class:"min-h-screen"},I={class:"flex justify-center"},Q={class:"mx-auto px-5"},q={class:"mb-8"},D={key:0,class:"text-gray-500"},H={key:0,class:"flex justify-end mr-4"},S=v({__name:"Index",setup(J){var l,d;const f=((d=(l=g)==null?void 0:l.value)==null?void 0:d.roles.indexOf("Admin"))>=0,m=k([]),p=b();m.value=p.getRoutes().filter(t=>{var e;return t.path.startsWith("/posts/")&&((e=t.meta)==null?void 0:e.frontmatter)}).map(t=>{var e;return{path:t.path,name:t.name,frontmatter:(e=t.meta)==null?void 0:e.frontmatter}}).filter(t=>!t.path.includes("employment-history")).sort((t,e)=>{var s,r,a;return(a=(s=e.frontmatter.date)!=null?s:"")==null?void 0:a.localeCompare((r=t.frontmatter.date)!=null?r:"")});const h=async()=>{p.push({path:"/posts/create"})},y=async()=>{let t=[];const e=await N.api(new V);return e.succeeded&&e.response.results&&(t=e.response.results.forEach(s=>{m.value.push({id:s.id,path:"/posts/"+s.path,name:s.name,frontmatter:{title:s.name,summary:"Add a summary property"}})})),t};return C(async()=>{await y()}),(t,e)=>{const s=x,r=E("router-link");return o(),n("div",F,[c("main",I,[c("div",Q,[i(s,{class:"my-8",name:"Blog"}),(o(!0),n(w,null,A(m.value,a=>(o(),n("div",q,[i(r,{class:"text-2xl hover:text-green-600",to:a.path},{default:P(()=>[R(_(a.frontmatter.title),1)]),_:2},1032,["to"]),a.frontmatter.summary?(o(),n("p",D,_(a.frontmatter.summary),1)):u("",!0)]))),256))])]),f?(o(),n("div",H,[c("button",{onClick:h},[i(B(j))])])):u("",!0)])}}});export{S as default};
