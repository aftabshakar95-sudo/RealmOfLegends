// Arena settings moved out of layout inline script
(function(){
    function toast(msg){
        const d=document.createElement('div'); d.textContent=msg;
        Object.assign(d.style,{position:'fixed',right:'20px',bottom:'20px',background:'rgba(0,0,0,0.8)',color:'#fff',padding:'8px 12px',borderRadius:'8px',zIndex:99999});
        document.body.appendChild(d); setTimeout(()=>d.remove(),2500);
    }

    function setEnabled(opts){
        try{ localStorage.setItem('arena.particles', opts.particles? '1':'0'); localStorage.setItem('arena.sfx', opts.sfx? '1':'0'); }catch{}
        if(window.ArenaEffects && typeof window.ArenaEffects.setEnabled==='function') window.ArenaEffects.setEnabled(opts);
    }

    // create small settings panel
    const panel=document.createElement('div'); panel.id='arenaSettingsPanel';
    Object.assign(panel.style,{position:'fixed',right:'20px',top:'70px',background:'#111',color:'#fff',padding:'12px',borderRadius:'8px',zIndex:99999,display:'none',boxShadow:'0 8px 24px rgba(0,0,0,0.6)'});
    panel.innerHTML = `
        <label style="display:flex;align-items:center;gap:8px;margin-bottom:8px;"><input type="checkbox" id="panelParticles"/> Enable Particles (Shift+P)</label>
        <label style="display:flex;align-items:center;gap:8px;"><input type="checkbox" id="panelSfx"/> Enable SFX (Shift+S)</label>
    `;
    document.body.appendChild(panel);

    const btn=document.getElementById('arenaSettingsToggle');
    if(btn){ btn.addEventListener('click', ()=>{ panel.style.display = panel.style.display==='none'?'block':'none'; }); }

    const pChk = panel.querySelector('#panelParticles');
    const sChk = panel.querySelector('#panelSfx');
    try{
        pChk.checked = localStorage.getItem('arena.particles') !== '0';
        sChk.checked = localStorage.getItem('arena.sfx') !== '0';
    }catch{}

    function apply(){ setEnabled({ particles: !!pChk.checked, sfx: !!sChk.checked }); }
    pChk.addEventListener('change', ()=>{ apply(); toast('Particles ' + (pChk.checked? 'enabled':'disabled')); });
    sChk.addEventListener('change', ()=>{ apply(); toast('SFX ' + (sChk.checked? 'enabled':'disabled')); });
    apply();

    // Keyboard shortcuts
    window.addEventListener('keydown', function(e){
        if(!e.shiftKey) return;
        if(e.key.toLowerCase()==='p'){ pChk.checked = !pChk.checked; pChk.dispatchEvent(new Event('change')); }
        if(e.key.toLowerCase()==='s'){ sChk.checked = !sChk.checked; sChk.dispatchEvent(new Event('change')); }
    });
})();
