import { test, expect } from '@playwright/test';

test('Home carrega e exibe seção Produtos', async ({ page }) => {
  await page.route('**/api/categoria', route => route.fulfill({
    status: 200,
    contentType: 'application/json',
    body: JSON.stringify([
      { id: '11111111-1111-1111-1111-111111111111', nome: 'Masculino' },
      { id: '22222222-2222-2222-2222-222222222222', nome: 'Feminino' }
    ])
  }));

  await page.route('**/api/produto**', route => route.fulfill({
    status: 200,
    contentType: 'application/json',
    body: JSON.stringify({
      Itens: [
        { id: 'p1', nome: 'Produto 1', preco: 99.9 },
        { id: 'p2', nome: 'Produto 2', preco: 49.9 }
      ],
      PaginaAtual: 1,
      TotalPaginas: 1,
      TotalItens: 2
    })
  }));

  await page.goto('/');
  await expect(page.getByRole('heading', { name: 'Produtos' })).toBeVisible();
  await expect(page.getByText('Produto 1')).toBeVisible();
  await expect(page.getByText('Produto 2')).toBeVisible();
});