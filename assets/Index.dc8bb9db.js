import{_ as x}from"./AppBreadcrumb.vue_vue_type_script_setup_true_lang.ffac0788.js";import{d as v,e as g,q as k,H as C,o,c as n,b as m,a as i,F as b,h as w,u as B,g as u,l as A,I as N,Q as V,w as P,i as R,t as _,J as E,r as F}from"./app.1930ab2b.js";import"./home.88331477.js";const I={class:"min-h-screen"},Q={class:"flex justify-center"},j={class:"px-5"},q={class:"mb-8"},D={key:0,class:"text-gray-500"},H={key:0,class:"ml-4 mb-4 mt-8"},S=v({__name:"Index",setup(J){var p,d;const f=((d=(p=g)==null?void 0:p.value)==null?void 0:d.roles.indexOf("Admin"))>=0,c=k([]),l=A();c.value=l.getRoutes().filter(t=>{var e;return t.path.startsWith("/posts/")&&((e=t.meta)==null?void 0:e.frontmatter)}).map(t=>{var e;return{path:t.path,name:t.name,frontmatter:(e=t.meta)==null?void 0:e.frontmatter}}).filter(t=>!t.path.includes("employment-history")).sort((t,e)=>{var s,r,a;return(a=(s=e.frontmatter.date)!=null?s:"")==null?void 0:a.localeCompare((r=t.frontmatter.date)!=null?r:"")});const h=async()=>{l.push({path:"/posts/create"})},y=async()=>{let t=[];const e=await N.api(new V);return e.succeeded&&e.response.results&&(t=e.response.results.forEach(s=>{c.value.push({id:s.id,path:"/posts/"+s.path,title:s.title,frontmatter:{title:s.title,summary:s.summary}})})),t};return C(async()=>{await y()}),(t,e)=>{const s=x,r=F("router-link");return o(),n("div",I,[m("main",Q,[m("div",j,[i(s,{class:"my-8",name:"Blog"}),(o(!0),n(b,null,w(c.value,a=>(o(),n("div",q,[i(r,{class:"text-2xl hover:text-green-600",to:a.path},{default:P(()=>[R(_(a.frontmatter.title),1)]),_:2},1032,["to"]),a.frontmatter.summary?(o(),n("p",D,_(a.frontmatter.summary),1)):u("",!0)]))),256))]),f?(o(),n("div",H,[m("button",{onClick:h},[i(B(E))])])):u("",!0)])])}}});export{S as default};
