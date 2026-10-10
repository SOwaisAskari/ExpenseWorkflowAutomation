const $ = id => document.getElementById(id);
async function api(url, options = {}) {
 const response = await fetch(url, { headers:{"Content-Type":"application/json",...(options.headers||{})}, ...options });
 const text = await response.text(); const data = text ? JSON.parse(text) : null;
 if (!response.ok) throw new Error(data?.message || data?.title || `Request failed (${response.status})`);
 return data;
}
const esc = value => String(value ?? "").replace(/[&<>"']/g,c=>({"&":"&amp;","<":"&lt;",">":"&gt;",'"':"&quot;","'":"&#39;"}[c]));
const date = value => value ? new Date(value).toLocaleString() : "—";
const money = (value,currency="PKR") => `${esc(currency)} ${Number(value||0).toLocaleString(undefined,{minimumFractionDigits:2,maximumFractionDigits:2})}`;
function badge(status){return `<span class="badge ${esc(String(status).toLowerCase())}">${esc(status.replace(/([A-Z])/g," $1").trim())}</span>`;}
function message(text,error=false){const el=$("message");el.hidden=false;el.textContent=text;el.className="message"+(error?" error":"");}
async function load(){
 try{
  const summary=await api("/api/dashboard/summary");
  $("total").textContent=summary.total;$("department").textContent=summary.awaitingDepartment;$("finance").textContent=summary.awaitingFinance;$("approved").textContent=summary.approved;
  const qs=new URLSearchParams();if($("filterStatus").value)qs.set("status",$("filterStatus").value);if($("filterType").value)qs.set("type",$("filterType").value);
  const rows=await api("/api/expenses"+(qs.size?"?"+qs.toString():""));
  $("rows").innerHTML=rows.length?rows.map(x=>`<tr><td><strong>EXP-${String(x.id).padStart(4,"0")}</strong><div>${esc(x.title)}</div></td><td>${esc(x.expenseType)}</td><td>${esc(x.department)}</td><td>${money(x.amount,x.currency)}</td><td>${badge(x.status)}</td><td>${date(x.updatedAtUtc)}</td><td><div class="actiongroup">${actions(x)}<button class="smallbtn" onclick="details(${x.id})">Details</button></div></td></tr>`).join(""):'<tr><td colspan="7" class="empty">No matching requests. Create a draft above to get started.</td></tr>';
 }catch(e){message(e.message,true);}
}
function actions(x){
 if(x.status==="Draft"||x.status==="Reverted")return `<button class="smallbtn" onclick="submitRequest(${x.id})">Submit</button>`;
 if(x.status==="PendingDepartment")return `<button class="smallbtn" onclick="decide(${x.id},'DepartmentHead','Approve')">Approve</button><button class="smallbtn" onclick="decide(${x.id},'DepartmentHead','Revert')">Revert</button>`;
 if(x.status==="PendingFinance")return `<button class="smallbtn" onclick="decide(${x.id},'Finance','Approve')">Approve</button><button class="smallbtn" onclick="decide(${x.id},'Finance','Revert')">Revert</button>`;
 return "";
}
$("createForm").addEventListener("submit",async e=>{
 e.preventDefault();
 try{
  const item=await api("/api/expenses",{method:"POST",body:JSON.stringify({
   expenseType:$("type").value,title:$("title").value.trim(),department:$("departmentName").value.trim(),
   requestedBy:$("requester").value.trim(),amount:Number($("amount").value),currency:$("currency").value.trim().toUpperCase(),
   justification:$("justification").value.trim()
  })});
  message(`Draft EXP-${String(item.id).padStart(4,"0")} created. Use Submit in the request register to start approval.`);
  $("createForm").reset();$("currency").value="PKR";await load();
 }catch(err){message(err.message,true);}
});
async function submitRequest(id){try{await api(`/api/expenses/${id}/submit`,{method:"POST",body:"{}"});message(`EXP-${String(id).padStart(4,"0")} submitted for department review.`);await load();}catch(e){message(e.message,true);}}
async function decide(id,actor,decision){
 const comment=prompt(`${actor} ${decision.toLowerCase()} comment:`,decision==="Approve"?"Reviewed in demo.":"Please update the request details.");
 if(comment===null)return;if(comment.trim().length<3){message("Enter a comment with at least 3 characters.",true);return;}
 try{await api(`/api/expenses/${id}/decision`,{method:"POST",body:JSON.stringify({actor,decision,comment:comment.trim()})});message(`EXP-${String(id).padStart(4,"0")} updated: ${decision} by ${actor}.`);await load();}
 catch(e){message(e.message,true);}
}
async function details(id){
 try{
  const x=await api(`/api/expenses/${id}`);
  $("modalTitle").textContent=`EXP-${String(x.id).padStart(4,"0")} · ${x.title}`;
  $("modalBody").innerHTML=`<div class="detailgrid">
   <div class="detail"><small>TYPE</small><strong>${esc(x.expenseType)}</strong></div><div class="detail"><small>STATUS</small>${badge(x.status)}</div>
   <div class="detail"><small>DEPARTMENT</small><strong>${esc(x.department)}</strong></div><div class="detail"><small>REQUESTED BY</small><strong>${esc(x.requestedBy)}</strong></div>
   <div class="detail"><small>AMOUNT</small><strong>${money(x.amount,x.currency)}</strong></div><div class="detail"><small>CREATED</small><strong>${date(x.createdAtUtc)}</strong></div>
   <div class="detail" style="grid-column:1/-1"><small>BUSINESS JUSTIFICATION</small>${esc(x.justification)}</div>
   ${x.decisionComment?`<div class="detail" style="grid-column:1/-1"><small>LATEST DECISION COMMENT</small>${esc(x.decisionComment)}</div>`:""}</div>
   <h3>Audit timeline</h3><div class="timeline">${(x.history||[]).map(ev=>`<div class="event"><strong>${esc(ev.toStatus.replace(/([A-Z])/g," $1").trim())}</strong><p>${esc(ev.comment)}</p><small>${esc(ev.actor)} · ${date(ev.createdAtUtc)}</small></div>`).join("")}</div>`;
  $("modal").hidden=false;
 }catch(e){message(e.message,true);}
}
$("closeModal").addEventListener("click",()=>$("modal").hidden=true);
$("modal").addEventListener("click",e=>{if(e.target===$("modal"))$("modal").hidden=true;});
$("refresh").addEventListener("click",load);$("filterStatus").addEventListener("change",load);$("filterType").addEventListener("change",load);load();
