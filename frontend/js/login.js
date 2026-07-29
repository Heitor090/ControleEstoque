document.addEventListener('DOMContentLoaded', () => {
    const token = localStorage.getItem('token');
    const authSection = document.getElementById('auth-section');
    const registerSection = document.getElementById('register-section');
    const mainContent = document.getElementById('main-content');
    const navMenu = document.getElementById('nav-menu');
    const btnLogout = document.getElementById('btn-logout');

    if (token) {
        if (authSection) authSection.style.display = 'none';
        if (registerSection) registerSection.style.display = 'none';
        if (mainContent) mainContent.style.display = 'block';
        if (navMenu) navMenu.style.display = 'inline-block';
        if (btnLogout) btnLogout.style.display = 'inline-block';

        carregarCatalogoHome();
    } else {
        if (authSection) authSection.style.display = 'block';
        if (registerSection) registerSection.style.display = 'none';
        if (mainContent) mainContent.style.display = 'none';
        if (navMenu) navMenu.style.display = 'none';
        if (btnLogout) btnLogout.style.display = 'none';
    }

    const loginForm = document.getElementById('login-form');
    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            const email = document.getElementById('email').value.trim();
            const senha = document.getElementById('senha').value;
            const errorMsg = document.getElementById('login-error');

            try {
                const response = await fetchWithToken(API_BASE_URL + '/api/Usuarios/autenticar', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ email, senha })
                });

                if (response.ok) {
                    const data = await response.json();
                    localStorage.setItem('token', data.token);
                    localStorage.setItem('usuario', JSON.stringify(data.usuario));
                    window.location.reload();
                } else {
                    const errorData = await response.json().catch(() => ({}));
                    if (errorMsg) {
                        errorMsg.textContent = errorData.message || 'Erro ao realizar login.';
                        errorMsg.style.display = 'block';
                    }
                }
            } catch (err) {
                if (errorMsg) {
                    errorMsg.textContent = 'Erro de conexão com o servidor.';
                    errorMsg.style.display = 'block';
                }
            }
        });
    }

    const registerForm = document.getElementById('register-form');
    if (registerForm) {
        registerForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            const nome = document.getElementById('reg-nome').value.trim();
            const email = document.getElementById('reg-email').value.trim();
            const senha = document.getElementById('reg-senha').value;
            const cpf = document.getElementById('reg-cpf').value.trim();
            const feedback = document.getElementById('register-feedback');

            try {
                const response = await fetchWithToken(API_BASE_URL + '/api/Usuarios/registrar-cliente', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ nome, email, senha, cpf })
                });

                if (response.ok) {
                    if (feedback) {
                        feedback.style.color = 'green';
                        feedback.textContent = 'Cadastro realizado com sucesso! Redirecionando para login...';
                    }
                    setTimeout(() => {
                        document.getElementById('email').value = email;
                        alternarTelaAuth('login');
                        if (feedback) feedback.textContent = '';
                    }, 1500);
                } else {
                    const errData = await response.json().catch(() => ({}));
                    if (feedback) {
                        feedback.style.color = 'var(--danger-color)';
                        feedback.textContent = errData.message || 'Erro ao realizar cadastro.';
                    }
                }
            } catch (err) {
                if (feedback) {
                    feedback.style.color = 'var(--danger-color)';
                    feedback.textContent = 'Erro de conexão com o servidor.';
                }
            }
        });
    }
});

function alternarTelaAuth(tela) {
    const authSection = document.getElementById('auth-section');
    const registerSection = document.getElementById('register-section');

    if (tela === 'cadastro') {
        if (authSection) authSection.style.display = 'none';
        if (registerSection) registerSection.style.display = 'block';
    } else {
        if (authSection) authSection.style.display = 'block';
        if (registerSection) registerSection.style.display = 'none';
    }
}

async function carregarCatalogoHome() {
    const welcomeMsg = document.getElementById('welcome-message');
    const catalogSec = document.getElementById('catalog-section');
    const container = document.getElementById('produtos-container');
    if (!catalogSec || !container) return;

    try {
        const response = await fetchWithToken(`${API_BASE_URL}/api/Produtos`);
        if (!response.ok) return;

        const produtos = await response.json();

        if (Array.isArray(produtos) && produtos.length > 0) {
            if (welcomeMsg) welcomeMsg.style.display = 'none';
            catalogSec.style.display = 'block';
            container.innerHTML = '';

            produtos.forEach(produto => {
                const card = document.createElement('div');
                card.className = 'product-card';

                const imgUrl = (produto.imagemUrl && produto.imagemUrl.trim() !== '') 
                    ? produto.imagemUrl 
                    : 'img/no-image.jpg';

                card.innerHTML = `
                    <img src="${imgUrl}" alt="${produto.nome}" onerror="this.src='img/no-image.jpg';">
                    <h4>${produto.nome}</h4>
                    <p>R$ ${produto.preco.toFixed(2)}</p>
                    <small>Estoque: ${produto.quantidadeEstoque}</small>
                    <br>
                    <button onclick='adicionarAoCarrinho(${JSON.stringify(produto)})' ${produto.quantidadeEstoque <= 0 ? 'disabled' : ''}>
                        ${produto.quantidadeEstoque <= 0 ? 'Esgotado' : 'Adicionar ao Carrinho'}
                    </button>
                `;
                container.appendChild(card);
            });
        } else {
            if (welcomeMsg) welcomeMsg.style.display = 'block';
            catalogSec.style.display = 'none';
        }
    } catch (err) {
        console.error('Erro ao carregar catálogo:', err);
    }
}

function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('usuario');
    localStorage.removeItem('carrinho');
    window.location.reload();
}
