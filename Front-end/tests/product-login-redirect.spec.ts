import { test, expect } from '@playwright/test';

test('Adicionar ao Carrinho sem login redireciona para /login', async ({ page }) => {
  await page.route('**/api/produto/*', route => route.fulfill({
    status: 200,
    contentType: 'application/json',
    body: JSON.stringify({ id: 'p1', nome: 'Tênis X', preco: 99.9 })
  }));

  await page.goto('/produto/p1');
  await page.getByRole('button', { name: 'Adicionar ao Carrinho' }).click();
  await expect(page).toHaveURL(/\/login$/);
});