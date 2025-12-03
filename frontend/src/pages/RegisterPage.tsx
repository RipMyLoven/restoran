import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { Select } from '../components/Select';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '../components/Card';
import { usersApi } from '../api';

export function RegisterPage() {
  const [formData, setFormData] = useState({
    username: '',
    password: '',
    confirmPassword: '',
    role: 'Waiter' as 'Admin' | 'Waiter' | 'Cook',
  });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (formData.password !== formData.confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    setIsLoading(true);

    try {
      await usersApi.create({
        username: formData.username,
        password: formData.password,
        role: formData.role,
      });
      setSuccess('Account created successfully! Please login.');
      setTimeout(() => navigate('/login'), 2000);
    } catch (err: any) {
      const errorData = err.response?.data;
      if (typeof errorData === 'string') {
        setError(errorData);
      } else if (errorData?.errors) {
        // Validation errors
        const messages = Object.values(errorData.errors).flat().join(', ');
        setError(messages);
      } else if (errorData?.title) {
        setError(errorData.title);
      } else {
        setError('Registration failed');
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-center">Create a new account</CardTitle>
      </CardHeader>
      <form onSubmit={handleSubmit}>
        <CardContent className="space-y-4">
          {error && (
            <div className="bg-red-50 text-red-500 p-3 rounded-md text-sm">
              {error}
            </div>
          )}
          {success && (
            <div className="bg-green-50 text-green-500 p-3 rounded-md text-sm">
              {success}
            </div>
          )}
          <Input
            label="Username"
            name="username"
            value={formData.username}
            onChange={handleChange}
            required
          />
          <Select
            label="Role"
            name="role"
            value={formData.role}
            onChange={handleChange}
            options={[
              { value: 'Waiter', label: 'Waiter' },
              { value: 'Cook', label: 'Cook' },
              { value: 'Admin', label: 'Admin' },
            ]}
          />
          <Input
            label="Password"
            type="password"
            name="password"
            value={formData.password}
            onChange={handleChange}
            required
          />
          <Input
            label="Confirm Password"
            type="password"
            name="confirmPassword"
            value={formData.confirmPassword}
            onChange={handleChange}
            required
          />
        </CardContent>
        <CardFooter className="flex flex-col space-y-4">
          <Button type="submit" className="w-full" disabled={isLoading}>
            {isLoading ? 'Creating account...' : 'Register'}
          </Button>
          <div className="text-sm text-center text-gray-600">
            Already have an account?{' '}
            <Link to="/login" className="text-blue-600 hover:text-blue-500">
              Sign in
            </Link>
          </div>
        </CardFooter>
      </form>
    </Card>
  );
}
