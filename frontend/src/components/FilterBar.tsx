import React from 'react';
import { Stack, Button, TextField, InputAdornment, IconButton } from '@mui/material';
import FilterAltIcon from '@mui/icons-material/FilterAlt';
import AddIcon from '@mui/icons-material/Add';
import SearchIcon from '@mui/icons-material/Search';

interface FilterBarProps {
  searchQuery: string;
  setSearchQuery: (query: string) => void;
}

export default function FilterBar({ searchQuery, setSearchQuery }: FilterBarProps) {
  return (
    <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}>
      <Stack direction="row" spacing={1} sx={{ flexWrap: 'wrap', gap: 1 }}>
        <Button variant="outlined" size="small" color="inherit">Mé filtry</Button>
        <Button variant="contained" size="small" color="info" disableElevation>Tento měsíc</Button>
        <Button variant="outlined" size="small" color="inherit">Region</Button>
        <Button variant="outlined" size="small" color="inherit">Tým</Button>
      </Stack>

      <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
        <TextField
          size="small"
          placeholder="Hledat obchodníka..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon fontSize="small" />
                </InputAdornment>
              ),
            }
          }}
          sx={{ bgcolor: 'white', minWidth: 200 }}
        />
        
        <IconButton color="info" sx={{ bgcolor: 'info.light', color: 'white', '&:hover': { bgcolor: 'info.main' } }}>
          <AddIcon />
        </IconButton>
        <Button variant="contained" color="info" startIcon={<FilterAltIcon />} disableElevation sx={{ whiteSpace: 'nowrap' }}>
          Filtrování
        </Button>
      </Stack>
    </Stack>
  );
}