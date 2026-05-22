import React from 'react';
import { Card, CardContent, Box, Typography, Avatar, Chip } from '@mui/material';
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
import TrendingDownIcon from '@mui/icons-material/TrendingDown';
import { Performer } from './types';

interface LeaderboardCardProps {
  person: Performer;
}

export default function LeaderboardCard({ person }: LeaderboardCardProps) {
  const formatValue = (val: number) => new Intl.NumberFormat('cs-CZ').format(val);

  return (
    <Card 
      variant="outlined" 
      sx={{ 
        borderRadius: 2, 
        borderColor: person.border,
        borderWidth: person.rank <= 3 ? 2 : 1,
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        transition: 'transform 0.2s, box-shadow 0.2s',
        '&:hover': { transform: 'translateY(-4px)', boxShadow: '0 4px 20px rgba(0,0,0,0.05)' }
      }}
    >
      <Box sx={{ bgcolor: person.bg, p: 1, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="subtitle2" sx={{ fontWeight: 'bold', color: person.rank <= 3 ? person.border : 'text.primary' }}>
          {person.rank}. MÍSTO
        </Typography>
        <Chip 
          size="small" 
          label={`${person.trend > 0 ? '+' : ''}${person.trend} %`} 
          icon={person.trend > 0 ? <TrendingUpIcon fontSize="small" /> : <TrendingDownIcon fontSize="small" />}
          color={person.trend > 0 ? 'success' : 'error'}
          variant="outlined"
          sx={{ bgcolor: 'white', fontWeight: 'bold' }}
        />
      </Box>
      
      <CardContent sx={{ textAlign: 'center', flexGrow: 1, pt: 3 }}>
        <Avatar src={person.avatarUrl || ''} sx={{ width: 64, height: 64, mx: 'auto', mb: 1 }}>
            {!person.avatarUrl && person.name.charAt(0)}
        </Avatar>
        <Typography variant="body1" sx={{ fontWeight: 'bold' }}>
          {person.name}
        </Typography>
      </CardContent>

      <Box sx={{ display: 'flex', borderTop: '1px solid #eee', textAlign: 'center', py: 1.5, bgcolor: '#fafafa' }}>
        <Box sx={{ flex: 1, borderRight: '1px solid #eee' }}>
          <Typography variant="h6" color="primary" sx={{ fontWeight: 'bold' }}>{person.deals}</Typography>
          <Typography variant="caption" color="textSecondary">dealů</Typography>
        </Box>
        <Box sx={{ flex: 1 }}>
          <Typography variant="subtitle1" sx={{ fontWeight: 'bold', whiteSpace: 'nowrap' }}>{formatValue(person.value)}</Typography>
          <Typography variant="caption" color="textSecondary">Kč</Typography>
        </Box>
      </Box>
    </Card>
  );
}