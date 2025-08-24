import LoginImage from '@/assets/images/Login.webp'
import Logo from '@/assets/images/Logogreen.svg'
import { authService } from '@/shared/constants/oidcConfig'
import { zodResolver } from '@hookform/resolvers/zod'
import { Button, Form, FormControl, FormField, FormItem, FormLabel, FormMessage, Input, Stack, useForm } from 'mada-design-system'
import { z } from 'zod'
import { useAuthStore } from '../store/authStore'
import { authService as apiAuthService } from '../services/authService'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { pathNames } from '@/shared/constants/pathNames'

const Login = () => {
    const login = useAuthStore(state => state.login)
    const [isLoading, setIsLoading] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const navigate = useNavigate()
    
    const handleSsoLogin = async () => {
        await authService.signin()
    }
    
    const formSchema = z
        .object({
            email: z
                .string({
                    required_error: 'Please enter your email',
                })
                .min(1, 'Email is required')
                .email('Please enter a valid email address'),
            password: z
                .string({
                    required_error: 'Please enter your password',
                })
                .min(1, 'Password is required')
                .min(6, 'Password must be at least 6 characters'),
        })
        .required({ email: true, password: true })
    type FormSchema = z.infer<typeof formSchema>

    const form = useForm<FormSchema>({ resolver: zodResolver(formSchema) })

    const onSubmit = async (data: FormSchema) => {
        try {
            setIsLoading(true)
            setError(null)
            
            console.log('Attempting login with:', data.email)
            
            // Call the real authentication API
            const result = await apiAuthService.login({
                email: data.email,
                password: data.password
            })
            
            console.log('Login API response:', result)
            
            // apiAuthService.login returns AuthResult directly, not wrapped in response
            if (result && result.accessToken && result.user) {
                console.log('Login successful, updating auth store with:', {
                    user: result.user,
                    accessToken: result.accessToken,
                    refreshToken: result.refreshToken,
                })
                
                // Update auth store with real user data and tokens
                login({
                    user: result.user,
                    accessToken: result.accessToken,
                    refreshToken: result.refreshToken,
                })
                
                console.log('Auth store updated successfully')
                
                // Redirect to dashboard after successful login
                navigate(pathNames.dashboard)
            } else {
                console.error('Login failed - invalid response structure:', result)
                setError('Login failed. Please check your credentials.')
            }
        } catch (err: unknown) {
            console.error('Login error:', err)
            setError('Network error occurred. Please try again.')
        } finally {
            setIsLoading(false)
        }
    }

    return (
        <Stack justifyContent={'center'} alignItems={'center'} className="px-space-04 py-space-06 xl:px-space-09 xl:py-space-05">
            <Stack direction="col" alignItems="center" justifyContent="center" gap={6} className="h-[730px] w-[490px] rounded-5 px-space-06">
                <img src={Logo} alt="me logo" className="h-auto w-[173px] self-center dark:hidden" />
                <Stack direction="col" alignItems="center" gap={2}>
                    <h1 className="title-01 font-bold text-card-foreground">Sign In to Ikhtibar</h1>
                    <p className="text-body-01 text-foreground-secondary">Enter your email and password to access your account</p>
                </Stack>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className="flex w-full flex-col items-center gap-space-05">
                        {error && (
                            <div className="w-full p-3 text-sm text-red-600 bg-red-50 border border-red-200 rounded-md">
                                {error}
                            </div>
                        )}
                        <FormField
                            control={form.control}
                            name="email"
                            defaultValue={'admin@ikhtibar.com'}
                            render={({ field }) => (
                                <FormItem className="w-full">
                                    <FormLabel>Email Address</FormLabel>
                                    <FormControl>
                                        <Input 
                                            type="email" 
                                            placeholder="Enter your email address" 
                                            {...field} 
                                            variant={'outline'} 
                                            disabled={isLoading}
                                        />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name="password"
                            defaultValue={'password'}
                            render={({ field }) => (
                                <FormItem className="w-full">
                                    <FormLabel>Password</FormLabel>
                                    <FormControl>
                                        <Input 
                                            type="password" 
                                            placeholder="Enter your password" 
                                            {...field} 
                                            variant={'outline'}
                                            disabled={isLoading}
                                        />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <Button 
                            variant="default" 
                            colors="primary" 
                            rounded={'default'} 
                            className="self-stretch"
                            type="submit"
                            disabled={isLoading}
                        >
                            {isLoading ? 'Signing in...' : 'Sign In'}
                        </Button>
                        <Button
                            variant="default"
                            colors="primary"
                            className="self-stretch"
                            rounded={'default'}
                            type="button"
                            disabled={isLoading}
                            onClick={() => {
                                handleSsoLogin()
                            }}>
                            Sign In with SSO
                        </Button>
                    </form>
                </Form>
            </Stack>
            <div className="relative hidden xl:block">
                <img src={LoginImage} alt="login" />
                <span className="bg-login absolute right-0 top-0 h-full w-full" />
                <div className="gap absolute bottom-24 right-20 flex flex-col text-primary-foreground">
                    <h1 className="title-03 font-bold">Welcome to Ikhtibar</h1>
                </div>
            </div>
        </Stack>
    )
}

export default Login
