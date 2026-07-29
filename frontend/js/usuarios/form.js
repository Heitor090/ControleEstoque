const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get('id');
let originalPerfil = null;

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('form-usuario');
    const selectPerfil = document.getElementById('perfil');
    if (form) {
        form.addEventListener('submit', salvarUsuario);
    }
    if (selectPerfil) {
        selectPerfil.addEventListener('change', atualizarCamposPerfil);
    }
    atualizarCamposPerfil();
    carregarUsuario();
});

function atualizarCamposPerfil() {
    const selectPerfil = document.getElementById('perfil');
    const campoCpf = document.getElementById('campo-cpf');
    const campoTurno = document.getElementById('campo-turno');
    const campoSetor = document.getElementById('campo-setor');

    if (!selectPerfil) return;
    const perfil = selectPerfil.value;
    if (campoCpf) campoCpf.style.display = perfil === 'Cliente' ? 'block' : 'none';
    if (campoTurno) campoTurno.style.display = perfil === 'Caixa' ? 'block' : 'none';
    if (campoSetor) campoSetor.style.display = perfil === 'Gerente' ? 'block' : 'none';
}

async function carregarUsuario() {
    if (id) {
        const titulo = document.getElementById('titulo-pagina');
        const selectPerfil = document.getElementById('perfil');
        if (titulo) titulo.innerText = 'Editar Usuário';

        try {
            const response = await fetchWithToken(`${API_BASE_URL}/api/Usuarios/${id}`);
            if (!response.ok) throw new Error('Erro ao carregar usuário');

            const u = await response.json();
            originalPerfil = u.perfil;

            const inputNome = document.getElementById('nome');
            const inputEmail = document.getElementById('email');
            const inputCpf = document.getElementById('cpf');
            const inputTurno = document.getElementById('turno');
            const inputSetor = document.getElementById('setor');

            if (inputNome) inputNome.value = u.nome;
            if (inputEmail) inputEmail.value = u.email;
            if (selectPerfil) selectPerfil.value = u.perfil;
            atualizarCamposPerfil();

            if (inputCpf && u.cpf) inputCpf.value = u.cpf;
            if (inputTurno && u.turno) inputTurno.value = u.turno;
            if (inputSetor && u.setor) inputSetor.value = u.setor;

        } catch (error) {
            console.error(error);
            const feedback = document.getElementById('feedback-message');
            if (feedback) feedback.textContent = 'Erro ao carregar usuário.';
        }
    }
}

async function salvarUsuario(e) {
    e.preventDefault();
    const feedback = document.getElementById('feedback-message');
    const selectPerfil = document.getElementById('perfil');
    const inputNome = document.getElementById('nome');
    const inputEmail = document.getElementById('email');
    const inputSenha = document.getElementById('senha');

    if (!selectPerfil || !inputNome || !inputEmail) return;

    const perfil = selectPerfil.value;
    const nome = inputNome.value.trim();
    const email = inputEmail.value.trim();
    const senha = inputSenha ? inputSenha.value : '';

    if (!nome || !email) {
        if (feedback) feedback.textContent = 'Preencha os campos obrigatórios.';
        return;
    }

    const inputCpf = document.getElementById('cpf');
    const inputTurno = document.getElementById('turno');
    const inputSetor = document.getElementById('setor');

    let extraData = '';
    if (perfil === 'Cliente') extraData = inputCpf ? inputCpf.value.trim() : '';
    else if (perfil === 'Caixa') extraData = inputTurno ? inputTurno.value.trim() : '';
    else if (perfil === 'Gerente') extraData = inputSetor ? inputSetor.value.trim() : '';

    try {
        // Se editando usuário e o perfil mudou, chama alteracao/promocao de perfil primeiro
        if (id && originalPerfil && originalPerfil !== perfil) {
            const resPerfil = await fetchWithToken(`${API_BASE_URL}/api/Usuarios/${id}/alterar-perfil?novoPerfil=${encodeURIComponent(perfil)}&extra=${encodeURIComponent(extraData)}`, {
                method: 'PUT'
            });
            if (!resPerfil.ok) {
                const errData = await resPerfil.json().catch(() => ({}));
                throw new Error(errData.message || 'Erro ao alterar perfil do usuário.');
            }
        }

        let endpoint = '';
        let body = {};

        if (perfil === 'Cliente') {
            endpoint = id ? 'atualizar-cliente' : 'registrar-cliente';
            body = { id: id ? parseInt(id, 10) : 0, nome, email, senha, cpf: extraData };
        } else if (perfil === 'Caixa') {
            endpoint = id ? 'atualizar-caixa' : 'registrar-caixa';
            body = { id: id ? parseInt(id, 10) : 0, nome, email, senha, turno: extraData };
        } else if (perfil === 'Gerente') {
            endpoint = id ? 'atualizar-gerente' : 'registrar-gerente';
            body = { id: id ? parseInt(id, 10) : 0, nome, email, senha, setor: extraData };
        }

        const method = id ? 'PUT' : 'POST';
        const url = `${API_BASE_URL}/api/Usuarios/${endpoint}`;

        const response = await fetchWithToken(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });

        if (!response.ok) {
            const errData = await response.json().catch(() => ({}));
            throw new Error(errData.message || 'Erro ao salvar usuário.');
        }

        window.location.href = 'index.html';
    } catch (error) {
        console.error(error);
        if (feedback) feedback.textContent = error.message;
    }
}
