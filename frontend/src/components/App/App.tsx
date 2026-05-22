import React, { useState, useEffect, useMemo } from 'react';
import { Box, Typography, CircularProgress, Alert } from '@mui/material';

// Importy komponent (zůstávají stejné jako minule)
import FilterBar from '../FilterBar';
import TopPerformers from '../TopPerformers';
import LeaderboardTable from '../LeaderboardTable';
import { Performer } from '../types';

export default function App() {
  const [searchQuery, setSearchQuery] = useState('');
  
  // Tady už neukládáme rawData (stovky obchodů), ale rovnou seznam obchodníků
  const [tradersFromBackend, setTradersFromBackend] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  // --- 1. STAŽENÍ DAT Z C# BACKENDU ---
  useEffect(() => {
    const fetchTraders = async () => {
      try {
        setIsLoading(true);
        // TADY BUDE URL TVÉHO C# .NET API (např. https://localhost:7123/api/traders)
        const response = await fetch('https://localhost:7123/api/traders'); 
        
        if (!response.ok) {
          throw new Error(`HTTP chyba! Status: ${response.status}`);
        }
        
        // Z .NETu už by mělo přijít rovnou pole obchodníků: [ { id: 1, firstName: "...", totalTradeAmount: ... }, ... ]
        const data = await response.json();
        setTradersFromBackend(data);
        setError(null);
      } catch (err: any) {
        console.error('Chyba při stahování dat:', err);
        setError('Nepodařilo se načíst data ze serveru.');
      } finally {
        setIsLoading(false);
      }
    };

    fetchTraders();
  }, []);

  // --- 2. ZPRACOVÁNÍ PRO DESIGN (Srovnání a výpočet podílu) ---
  const processedData = useMemo(() => {
    if (!tradersFromBackend || tradersFromBackend.length === 0) return [];

    // Spočítáme celkovou částku celé firmy pro výpočet procentuálního "podílu"
    const totalCompanyValue = tradersFromBackend.reduce((sum, trader) => sum + (trader.totalTradeAmount || 0), 0);

    // Seřadíme obchodníky podle částky sestupně
    const sortedTraders = [...tradersFromBackend].sort((a, b) => b.totalTradeAmount - a.totalTradeAmount);

    const topColors = [
      { border: '#ffb300', bg: '#fff8e1' },
      { border: '#90a4ae', bg: '#eceff1' },
      { border: '#ff8a65', bg: '#fbe9e7' },
    ];

    // Přemapujeme data z C# do formátu, který vyžadují naše React komponenty
    return sortedTraders.map((trader, index): Performer => ({
      rank: index + 1,
      // Spojíme FirstName a LastName do jednoho jména
      name: `${trader.firstName || ''} ${trader.lastName || ''}`.trim() || 'Neznámý obchodník',
      avatarUrl: null, // V C# zatím avatar není, takže se ukážou automatické iniciály
      deals: trader.dealCount || 0,
      value: trader.totalTradeAmount || 0,
      trend: Math.floor(Math.random() * 25) - 10, // Trend zatím náhodný (dokud nebude v DB historie)
      share: totalCompanyValue > 0 ? Math.round(((trader.totalTradeAmount || 0) / totalCompanyValue) * 100) : 0,
      border: index < 3 ? topColors[index].border : '#e0e0e0',
      bg: index < 3 ? topColors[index].bg : '#ffffff'
    }));
  }, [tradersFromBackend]);

  // --- 3. FILTROVÁNÍ A ROZDĚLENÍ KARET/TABULKY ---
  const { topPerformers, listPerformers } = useMemo(() => {
    const filtered = processedData.filter((p) =>
      p.name.toLowerCase().includes(searchQuery.toLowerCase())
    );
    return {
      topPerformers: filtered.slice(0, 6),
      listPerformers: filtered.slice(6)
    };
  }, [processedData, searchQuery]);

  // Vykreslení Loading/Error stavů
  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', bgcolor: '#fcfcfc' }}>
        <CircularProgress color="info" />
      </Box>
    );
  }

  if (error) {
    return (
      <Box sx={{ p: 4, bgcolor: '#fcfcfc', minHeight: '100vh' }}>
        <Alert severity="error">{error}</Alert>
      </Box>
    );
  }

  return (
    <Box sx={{ p: { xs: 2, md: 4 }, backgroundColor: '#fcfcfc', minHeight: '100vh', fontFamily: 'sans-serif' }}>
      
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold' }}>
          Žebříček obchodníků
        </Typography>
        <FilterBar searchQuery={searchQuery} setSearchQuery={setSearchQuery} />
      </Box>

      <TopPerformers performers={topPerformers} />
      <LeaderboardTable performers={listPerformers} />

      {processedData.length > 0 && topPerformers.length === 0 && listPerformers.length === 0 && (
        <Box sx={{ textAlign: 'center', py: 8 }}>
          <Typography variant="h6" color="textSecondary">
            Nenalezeni žádní obchodníci odpovídající hledání "{searchQuery}".
          </Typography>
        </Box>
      )}
      
      {!isLoading && !error && processedData.length === 0 && (
         <Box sx={{ textAlign: 'center', py: 8 }}>
           <Typography variant="h6" color="textSecondary">
             V systému nejsou zatím žádní obchodníci.
           </Typography>
         </Box>
      )}
    </Box>
  );
}