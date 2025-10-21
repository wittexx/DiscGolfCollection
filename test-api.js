// Simple test to verify API connectivity
const API_BASE_URL = 'http://localhost:5000/api';

async function testAPI() {
    console.log('Testing API connectivity...');
    
    try {
        // Test GET all discs
        console.log('1. Testing GET /api/discs...');
        const response = await fetch(`${API_BASE_URL}/discs`);
        
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }
        
        const discs = await response.json();
        console.log('✅ GET /api/discs successful:', discs);
        
        // Test POST new disc
        console.log('2. Testing POST /api/discs...');
        const newDisc = {
            name: "Test API Disc",
            category: 1,
            brand: "API Test",
            description: "Testing API from script",
            speed: 7,
            glide: 5,
            turn: -1,
            fade: 2,
            plastic: "Star",
            color: "Red",
            weight: 175
        };
        
        const postResponse = await fetch(`${API_BASE_URL}/discs`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(newDisc)
        });
        
        if (!postResponse.ok) {
            throw new Error(`HTTP ${postResponse.status}: ${postResponse.statusText}`);
        }
        
        const createdDisc = await postResponse.json();
        console.log('✅ POST /api/discs successful:', createdDisc);
        
        console.log('🎉 All API tests passed!');
        
    } catch (error) {
        console.error('❌ API test failed:', error.message);
        console.error('Full error:', error);
    }
}

testAPI();
