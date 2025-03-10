import React from 'react'
import { Button } from './ui/button';
import { toast } from 'sonner';
import { useTheme } from 'next-themes';

export default function ThemeToggle() {  
  const { theme, setTheme} = useTheme();  

  function toggleTheme() {    
    setTheme(theme === "light" ? "dark" : "light");  
    toast(`Set theme to ${theme} mode`);
  }

  return (
    <div className='flex flex-row gap-2 items-center'>
      <Button onClick={toggleTheme}>Toggle Theme</Button>
      <span>{theme}</span>
    </div>
  )
}
