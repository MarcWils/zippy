(()=>{"use strict";var e={236:(e,t)=>{Object.defineProperty(t,"__esModule",{value:!0}),t.ZipFile=void 0,t.ZipFile=class{constructor(e){this.file=e,this.fileName=e.name}validate(){this.file.slice(0,4).arrayBuffer().then((e=>this.onSignatureRead(e)))}onSignatureRead(e){console.log(this.toHexString(new Uint8Array(e)))}toHexString(e){return[...e].map((e=>e.toString(16).padStart(2,"0"))).join("")}}}},t={};function i(n){var r=t[n];if(void 0!==r)return r.exports;var o=t[n]={exports:{}};return e[n](o,o.exports,i),o.exports}(()=>{const e=i(236),t=document.getElementById("zipFileSelector"),n=document.getElementById("zipFileContents");t&&t.addEventListener("change",(()=>{!function(t){if(n.classList.remove("hidden"),1!==t.length)return;const i=new e.ZipFile(t[0]);document.getElementById("zipFileName").innerText=i.fileName,i.validate()}(t.files)}))})()})();
//# sourceMappingURL=site.js.map