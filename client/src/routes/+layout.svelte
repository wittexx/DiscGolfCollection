<script lang="ts">
	import favicon from '$lib/assets/favicon.svg';
	import { theme, localStorageManager, type Theme } from '$lib/stores.js';
	import { onMount } from 'svelte';
	import '../app.css';

	let { children } = $props();
	let currentTheme = $state<Theme>('light');

	onMount(() => {
		currentTheme = localStorageManager.getTheme();
		theme.subscribe(newTheme => {
			currentTheme = newTheme;
		});
	});

	function toggleTheme() {
		const newTheme = currentTheme === 'light' ? 'dark' : 'light';
		localStorageManager.setTheme(newTheme);
	}
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>

<div class="app">
	<header class="app-header">
		<button class="theme-toggle" onclick={toggleTheme} title="Toggle {currentTheme === 'light' ? 'dark' : 'light'} mode">
			{#if currentTheme === 'light'}
				🌙
			{:else}
				☀️
			{/if}
		</button>
	</header>
	
	<main class="app-content">
		{@render children?.()}
	</main>
</div>

