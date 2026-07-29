function checkAuthentication() {
    const token = localStorage.getItem('token');
    const isRootIndex = !window.location.pathname.includes('/html/');

    if (!token) {
        if (!isRootIndex) {
            window.location.href = '../../index.html?unauthorized=true';
        }
        return;
    }

    const usuarioRaw = localStorage.getItem('usuario');
    const usuario = usuarioRaw ? JSON.parse(usuarioRaw) : null;
    const perfil = usuario?.perfil || 'Cliente';
    const path = window.location.pathname;

    // Proteção de Rota por Perfil (Redireciona se tentar acessar URL restrita)
    if (perfil === 'Cliente') {
        if (path.includes('/fornecedores/') || path.includes('/formaspagamento/') || path.includes('/usuarios/')) {
            window.location.href = isRootIndex ? 'index.html' : '../../index.html';
            return;
        }
    } else if (perfil === 'Caixa') {
        if (path.includes('/usuarios/')) {
            window.location.href = isRootIndex ? 'index.html' : '../../index.html';
            return;
        }
    }

    configurarMenuNavegacao();
}

function configurarMenuNavegacao() {
    const usuarioRaw = localStorage.getItem('usuario');
    const usuario = usuarioRaw ? JSON.parse(usuarioRaw) : null;
    const perfil = usuario?.perfil || 'Cliente';

    const navLinks = document.querySelectorAll('nav a');
    navLinks.forEach(link => {
        const href = link.getAttribute('href') || '';

        // Filtragem por Perfil
        if (href.includes('fornecedores') || href.includes('formaspagamento') || href.includes('usuarios')) {
            if (perfil === 'Cliente') {
                link.style.display = 'none';
            } else if (href.includes('usuarios') && perfil === 'Caixa') {
                link.style.display = 'none';
            } else {
                link.style.display = 'inline-block';
            }
        }

        // Renomear Contas a Receber para Contas a Pagar no perfil Cliente
        if (href.includes('contasreceber')) {
            if (perfil === 'Cliente') {
                link.textContent = 'Contas a Pagar';
            } else {
                link.textContent = 'Contas a Receber';
            }
        }
    });
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', checkAuthentication);
} else {
    checkAuthentication();
}