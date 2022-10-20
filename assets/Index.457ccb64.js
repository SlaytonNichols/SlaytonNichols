import{_ as b}from"./FormLoading.vue_vue_type_script_setup_true_lang.aa16d375.js";import{_ as B}from"./AppBreadcrumb.vue_vue_type_script_setup_true_lang.7b3f1b95.js";import{d as C,e as P,q as y,H as w,I as A,o as n,c as r,b as m,a as d,f as I,g as f,F as j,h as N,u as V,l as E,w as F,i as L,t as g,r as R,_ as S}from"./app.3c450ce0.js";import{u as $,A as q}from"./posts.e0077577.js";import"./home.5231e2d1.js";const D={class:"min-h-screen"},H={class:"flex"},M={class:"px-5 flex-grow mt-10"},O={class:"flex mb-8 justify-center flex-col align-center"},T={key:0,class:"text-gray-500"},W={key:0,class:"m-4 justify-end"},z=C({__name:"Index",setup(G){var v,x;const _=$(),u=((x=(v=P)==null?void 0:v.value)==null?void 0:x.roles.indexOf("Admin"))>=0,a=y([]),p=y(),c=w({get(){return p.value},set(e){p.value=e}}),h=E();a.value=h.getRoutes().filter(e=>{var s;return e.path.startsWith("/posts/")&&((s=e.meta)==null?void 0:s.frontmatter)}).map(e=>{var s;return{path:e.path,name:e.name,frontmatter:(s=e.meta)==null?void 0:s.frontmatter}}).filter(e=>!e.path.includes("employment-history")).sort((e,s)=>{var o,t,i;return(i=(o=s.frontmatter.date)!=null?o:"")==null?void 0:i.localeCompare((t=e.frontmatter.date)!=null?t:"")});const k=async()=>{h.push({path:"/posts/create"})};return A(async()=>{c.set(!0),await _.refreshPosts(),_.allPosts.forEach(t=>{a.value.unshift({id:t.id,path:"/posts/"+t.path,title:t.title,draft:t.draft,frontmatter:{title:t.title,summary:t.summary}})});let e=["todos"],s=a.value.filter(t=>e.includes(t.path)),o=a.value.filter(t=>!e.includes(t.path));a.value=s.concat(o),u||(a.value=a.value.filter(t=>!t.draft)),c.set(!1)}),(e,s)=>{const o=B,t=b,i=R("router-link");return n(),r("div",D,[m("main",H,[m("div",M,[d(o,{class:"my-4 justify-center",name:"Blog"}),c.get()?(n(),I(t,{key:0,class:"justify-center",loading:c.get(),icon:!0,text:""},null,8,["loading"])):f("",!0),(n(!0),r(j,null,N(a.value,l=>(n(),r("div",O,[d(i,{class:"text-2xl hover:text-green-600",to:l.path},{default:F(()=>[L(g(l.frontmatter.title),1)]),_:2},1032,["to"]),l.frontmatter.summary?(n(),r("p",T,g(l.frontmatter.summary),1)):f("",!0)]))),256))]),u?(n(),r("div",W,[m("button",{onClick:k},[d(V(q),{class:"w-8 h-8"})])])):f("",!0)])])}}});const Y=S(z,[["__scopeId","data-v-2fb8cdfc"]]);export{Y as default};
